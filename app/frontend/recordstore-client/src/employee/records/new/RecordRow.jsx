import React from 'react';

import useAuth from '../../../hooks/useAuth';

import { Link } from 'react-router-dom';

import { Tr, Td, Badge, IconButton } from '@chakra-ui/react';

import { FaEdit, FaChevronDown, FaChevronUp, FaTrashAlt } from 'react-icons/fa';

const RecordRow = ({ record, onEdit, onDelete }) => {
  const { user } = useAuth();

  return (
    <Tr
      key={record.id}
      cursor='pointer'
      _hover={{ bg: 'teal.50' }}
      transition='background-color 0.3s ease, transform 0.3s ease'
    >
      <Td>
        <Badge colorScheme='teal' mr={2}>
          {record.id}
        </Badge>
      </Td>
      <Td>{record.title}</Td>
      <Td>{record.releaseDate}</Td>
      <Td isNumeric>
        <IconButton
          aria-label='Edit'
          icon={<FaEdit />}
          colorScheme='teal'
          variant='ghost'
          size='sm'
          as={Link}
          to={`/records/${record.id}/edit`}
        />
        {user && user.role === 'admin' && (
          <>
            <IconButton
              aria-label='Delete'
              icon={<FaTrashAlt />}
              colorScheme='red'
              variant='ghost'
              size='sm'
              onClick={() => onDelete(record.id)}
            />
          </>
        )}
      </Td>
    </Tr>
  );
};

export default RecordRow;
