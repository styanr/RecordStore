import React, { useState, useEffect } from 'react';
import {
  Box,
  Heading,
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Spinner,
  Container,
  TableContainer,
  Button,
  TableCaption,
  Text,
  HStack,
  InputGroup,
  InputLeftElement,
  Input,
  InputRightAddon,
  IconButton,
  useToast,
  useDisclosure,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  ModalFooter,
} from '@chakra-ui/react';

import { FaPlus, FaSearch, FaTrashAlt } from 'react-icons/fa';

import { Link } from 'react-router-dom';
import useRecords from '../../../hooks/useRecords';
import usePages from '../../../hooks/usePages';

import RecordRow from './RecordRow';

const Records = () => {
  const { getRecords, deleteRecord } = useRecords();
  const [records, setRecords] = useState([]);
  const [loading, setLoading] = useState(true);
  const [title, setTitle] = useState('');
  const [recordIdToDelete, setRecordIdToDelete] = useState(null);
  const [isDeleteLoading, setIsDeleteLoading] = useState(false);

  const { page, totalPages, setTotalPages, nextPage, prevPage, goToPage } =
    usePages();

  const { isOpen, onOpen, onClose } = useDisclosure();

  useEffect(() => {
    const fetchRecords = async () => {
      setLoading(true);
      try {
        const data = await getRecords({ page, title });
        setRecords(data.results);

        setTotalPages(data.pageCount);
      } catch (error) {
        console.error('Error fetching records:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchRecords();
  }, [page, title]);

  const handleDeleteClick = (recordId) => {
    console.log('Delete record:', recordId);
    setRecordIdToDelete(recordId);
    onOpen();
  };

  const handleDelete = async () => {
    setIsDeleteLoading(true);
    try {
      await deleteRecord(recordIdToDelete);
      setRecords(records.filter((record) => record.id !== recordIdToDelete));
      toast({
        title: 'Запис успішно видалено',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    } catch (error) {
      console.error('Error deleting record:', error);
      toast({
        title: 'Помилка',
        description: 'Не вдалося видалити запис. ' + error.message,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    } finally {
      setIsDeleteLoading(false);
      onClose();
    }
  };
  return (
    <Box bg='gray.100' py={12} flexGrow={1}>
      <Container maxW='7xl'>
        <Heading color='teal.600' mb={4}>
          Записи
        </Heading>
        <Button
          colorScheme='teal'
          leftIcon={<FaPlus />}
          _hover={{ bg: 'teal.700' }}
          transition='background-color 0.3s ease'
          as={Link}
          to='/records/new'
        >
          Додати запис
        </Button>
        <Text mt={4} fontSize='sm' color='teal.600'>
          Сторінка {page} з {totalPages}
        </Text>
        <InputGroup my={4} backgroundColor='white' borderRadius='lg'>
          <InputLeftElement pointerEvents='none'>
            <FaSearch color='gray.300' />
          </InputLeftElement>
          <Input
            placeholder='Пошук...'
            value={title}
            onChange={(e) => setTitle(e.target.value)}
          />
        </InputGroup>
        {loading ? (
          <Box
            display='flex'
            justifyContent='center'
            alignItems='center'
            height='100vh'
          >
            <Spinner size='xl' color='teal.500' />
          </Box>
        ) : (
          <Box mt={8}>
            <TableContainer
              boxShadow='md'
              borderRadius='lg'
              overflow='hidden'
              backgroundColor='white'
            >
              <Table>
                <TableCaption
                  color='teal.600'
                  fontSize='sm'
                  fontWeight='bold'
                  textTransform='uppercase'
                  letterSpacing='wide'
                  py={2}
                >
                  Список записів
                </TableCaption>
                <Thead>
                  <Tr>
                    <Th>Ідентифікатор</Th>
                    <Th>Назва</Th>
                    <Th>Дата виходу</Th>
                    <Th isNumeric>Дії</Th>
                  </Tr>
                </Thead>
                <Tbody>
                  {records.map((record) => (
                    <RecordRow
                      key={record.id}
                      record={record}
                      onDelete={handleDeleteClick}
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
        )}
      </Container>

      <Modal isOpen={isOpen} onClose={onClose} isCentered>
        <ModalOverlay
          bg='blackAlpha.300'
          backdropFilter='blur(4px) hue-rotate(90deg)'
          transition='all 0.3s ease'
        />
        <ModalContent>
          {isDeleteLoading ? (
            <ModalBody>
              <Box
                h='10rem'
                display={'flex'}
                justifyContent={'center'}
                alignItems={'center'}
              >
                <Spinner size='xl' color='teal.500' />
              </Box>
            </ModalBody>
          ) : (
            <>
              <ModalHeader color='teal.600'>
                Підтвердження видалення
              </ModalHeader>
              <ModalCloseButton />
              <ModalBody>
                <Box textAlign='center' display={'flex'} flexDirection={'row'}>
                  <FaTrashAlt size={48} color='red.500' mb={4} />
                  <Box>
                    <Heading size='md' mb={2}>
                      Ви дійсно хочете видалити цей запис?
                    </Heading>
                    <Text>
                      Продукти, які пов'язані з цим записом, також будуть
                      видалені. Цю дію не можна буде скасувати.
                    </Text>
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
            </>
          )}
        </ModalContent>
      </Modal>
    </Box>
  );
};

export default Records;
