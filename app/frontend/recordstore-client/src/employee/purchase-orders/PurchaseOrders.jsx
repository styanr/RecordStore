import {
  Box,
  Button,
  Container,
  Heading,
  Table,
  Thead,
  Tbody,
  Th,
  Tr,
  TableCaption,
  TableContainer,
  Spinner,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  useDisclosure,
  Text,
  HStack,
  useToast,
} from '@chakra-ui/react';
import { FaPlus, FaTrashAlt } from 'react-icons/fa';
import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

import usePurchaseOrders from './usePurchaseOrders';
import usePages from '../../hooks/usePages';

import PurchaseOrder from './PurchaseOrder';

const PurchaseOrders = () => {
  const toast = useToast();

  const [purchaseOrdersParams, setPurchaseOrdersParams] = useState({
    orderBy: 'orderDate',
    orderDirection: 'desc',
    page: 1,
  });
  const [supplierParams, setSupplierParams] = useState({});
  const {
    purchaseOrders,
    isPurchaseOrderLoading,
    purchaseOrderError,
    deletePurchaseOrder,
  } = usePurchaseOrders(purchaseOrdersParams, supplierParams);

  const { isOpen, onOpen, onClose } = useDisclosure();
  const [orderToDelete, setOrderToDelete] = useState(null);

  const { page, totalPages, setTotalPages, nextPage, prevPage, goToPage } =
    usePages();

  const [isDeleteLoading, setIsDeleteLoading] = useState(false);

  useEffect(() => {
    setTotalPages(purchaseOrders.pageCount);
  }, [purchaseOrders.pageCount]);

  useEffect(() => {
    setPurchaseOrdersParams((prevParams) => ({
      ...prevParams,
      page,
    }));
  }, [page]);

  const handleDeleteClick = (order) => {
    setOrderToDelete(order.id);
    onOpen();
  };

  const handleDelete = async () => {
    setIsDeleteLoading(true);

    const response = await deletePurchaseOrder(orderToDelete.id);
    if (response.success) {
      toast({
        title: 'Закупівлю успішно видалено',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    } else {
      toast({
        title: 'Помилка видалення закупівлі',
        description: response.message,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
      onClose();
    }

    setIsDeleteLoading(false);
  };

  const handleExpand = (order, expanded) => {
    // Perform any necessary operations when expanding/collapsing a purchase order row
    console.log(`${order.id} is ${expanded ? 'expanded' : 'collapsed'}`);
  };

  if (isPurchaseOrderLoading) {
    return (
      <Box
        display='flex'
        justifyContent='center'
        alignItems='center'
        flexGrow={1}
      >
        <Spinner
          size='xl'
          color='teal.500'
          thickness='4px'
          speed='0.65s'
          emptyColor='gray.200'
        />
      </Box>
    );
  }

  return (
    <Box bg='gray.100' py={12} flexGrow={1}>
      <Container maxW='7xl'>
        <Heading color='teal.600' mb={4}>
          Керування закупівлями
        </Heading>
        <Button
          colorScheme='teal'
          leftIcon={<FaPlus />}
          _hover={{ bg: 'teal.700' }}
          transition='background-color 0.3s ease'
          as={Link}
          to='/purchase-orders/new'
        >
          Створити нову закупівлю
        </Button>
        <Box mt={8}>
          <TableContainer
            boxShadow='md'
            borderRadius='lg'
            overflow='hidden'
            backgroundColor='white'
          >
            <Table variant='simple'>
              <TableCaption
                color='teal.600'
                fontSize='sm'
                fontWeight='bold'
                textTransform='uppercase'
                letterSpacing='wide'
                py={2}
              >
                Список закупівель
              </TableCaption>
              <Thead>
                <Tr>
                  <Th>Замовлення</Th>
                  <Th>Дата</Th>
                  <Th>Постачальник</Th>
                  <Th isNumeric>Сума</Th>
                  <Th></Th>
                </Tr>
              </Thead>
              <Tbody>
                {purchaseOrders.results.map((purchaseOrder) => (
                  <PurchaseOrder
                    key={purchaseOrder.id}
                    purchaseOrder={purchaseOrder}
                    onDelete={handleDeleteClick}
                    onExpand={handleExpand}
                  />
                ))}
              </Tbody>
            </Table>
          </TableContainer>
          <HStack mt={4} justify='center'>
            <Button
              isDisabled={page === 1}
              onClick={() => prevPage()}
              size='sm'
            >
              Попередня
            </Button>
            <Text>{page}</Text>
            <Button
              isDisabled={page === totalPages}
              onClick={() => nextPage()}
              size='sm'
            >
              Наступна
            </Button>
          </HStack>
        </Box>
      </Container>

      <Modal isOpen={isOpen} onClose={onClose} isCentered>
        <ModalOverlay
          bg='blackAlpha.300'
          backdropFilter='blur(4px) hue-rotate(90deg)'
          transition='all 0.3s ease'
        />
        <ModalContent>
          <ModalHeader color='teal.600'>Підтвердження видалення</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Box textAlign='center' display={'flex'} flexDirection={'row'}>
              <FaTrashAlt size={48} color='red.500' mb={4} />
              <Box>
                <Heading size='md' mb={2}>
                  Ви дійсно хочете видалити цю закупівлю?
                </Heading>
                <Text>Цю дію не можна буде скасувати.</Text>
              </Box>
            </Box>
          </ModalBody>
          <ModalFooter>
            <Button variant='ghost' mr={3} onClick={onClose}>
              Скасувати
            </Button>
            <Button
              colorScheme='red'
              onClick={handleDelete}
              _hover={{ bg: 'red.600' }}
              transition='background-color 0.3s ease'
            >
              Видалити
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </Box>
  );
};

export default PurchaseOrders;
